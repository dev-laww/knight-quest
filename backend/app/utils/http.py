import importlib
import traceback
import typing
from typing import Any, Awaitable, Dict, List, Optional, Union

import humps
from fastapi import HTTPException, status, FastAPI
from fastapi.logger import logger
from fastapi.responses import JSONResponse
from fastapi.responses import Response as FastAPIResponse
from starlette.background import BackgroundTask

from ..schemas.response import Response as ResponseSchema
from ..settings import AppMode, settings


class ResponseBuilder:
    @staticmethod
    def build(
        message: str,
        success: bool = True,
        data: Optional[Union[Dict[str, Any], List[Dict[str, Any]]]] = None
    ) -> ResponseSchema:
        return ResponseSchema(
            message=message,
            success=success,
            data=humps.camelize(data) if data is not None else None,
        )

    @staticmethod
    def build_success(
        message: str,
        data: Optional[Union[Dict[str, Any], List[Dict[str, Any]]]] = None
    ) -> ResponseSchema:
        return ResponseSchema(
            message=message,
            data=humps.camelize(data) if data is not None else None,
        )

    @staticmethod
    def build_unauthorized(message: Optional[str] = None) -> HTTPException:
        return HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail=message,
            headers={"WWW-Authenticate": "Bearer"},
        )

    @staticmethod
    def build_forbidden(message: Optional[str] = None) -> HTTPException:
        return HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail=message,
        )

    @staticmethod
    def build_not_found(message: Optional[str] = None) -> HTTPException:
        return HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=message,
        )

    @staticmethod
    def build_bad_request(message: Optional[str] = None) -> HTTPException:
        return HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=message,
        )

    @staticmethod
    def build_internal_server_error(message: Optional[str] = None) -> HTTPException:
        return HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=message,
        )

    @staticmethod
    def build_error(message: str, status_code: int = status.HTTP_400_BAD_REQUEST) -> HTTPException:
        return HTTPException(
            status_code=status_code,
            detail=message,
        )

    @staticmethod
    def exception(exc: HTTPException, **kwargs) -> Union[FastAPIResponse, Awaitable[ResponseSchema]]:
        data = {
            **kwargs
        }

        if exc.status_code == status.HTTP_500_INTERNAL_SERVER_ERROR and settings.app_mode == AppMode.DEVELOPMENT:
            data.update({
                'traceback': traceback.format_exc().splitlines()
            })

        response: FastAPIResponse = JSONResponse(
            status_code=exc.status_code,
            content={
                'success': False,
                'message': (
                    'An error occurred.'
                    if settings.app_mode == AppMode.PRODUCTION and exc.status_code != status.HTTP_500_INTERNAL_SERVER_ERROR else
                    exc.detail
                ),
                **data
            },
        )

        return response


class Response(JSONResponse):
    def __init__(
        self,
        content: typing.Dict,
        status_code: int = 200,
        headers: typing.Mapping[str, str] | None = None,
        media_type: str | None = None,
        background: BackgroundTask | None = None,
    ):
        data = content.copy()
        for k, v in content.items():
            if v is None:
                data.pop(k)

        super().__init__(
            data,
            status_code,
            headers,
            media_type,
            background
        )


class Routes:
    __routers = [
        'app.routers.authentication'
    ]

    @staticmethod
    def load(app: FastAPI):
        logger.info('Loading routes...')

        for router in Routes.__routers:
            module = importlib.import_module(router)

            if not hasattr(module, 'router'):
                print(f'Module {router} has no attribute \'router\'. Skipping...')
                continue

            app.include_router(module.router)
