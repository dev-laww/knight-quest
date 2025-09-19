import logging
from typing import Awaitable, Union

from fastapi import FastAPI, HTTPException, Request, Response, status
from fastapi.middleware.cors import CORSMiddleware
from pydantic import create_model
from starlette.exceptions import HTTPException as StarletteHTTPException
from starlette.middleware.sessions import SessionMiddleware

from . import http
from .logging import logger
from ..settings import AppMode, settings

logger.level = logging.DEBUG if settings.app_mode == AppMode.DEVELOPMENT else logging.INFO


# @asynccontextmanager
# async def lifespan(app: FastAPI):
#     logger.info('Connecting to database...')
#     await database.connect()
#
#     route: APIRoute
#
#     for route in app.routes:  # type: ignore
#         endpoint = route.endpoint
#
#         if endpoint and hasattr(endpoint, '__schema_name__'):
#             route.body_field.type_.__name__ = endpoint.__schema_name__  # type: ignore
#
#     # start background tasks
#     asyncio.create_task(BookingQueue().dispatch())  # noqa
#     asyncio.create_task(BookingQueue().process_all())  # noqa
#
#     yield  # Lifespan context
#
#     logger.info('Disconnecting from database...')
#     await database.disconnect()


def setup_app(app: FastAPI):
    app.add_middleware(
        CORSMiddleware,  # type: ignore
        allow_origins=["*"],
        allow_credentials=True,
        allow_methods=["*"],
        allow_headers=["*"],
    )

    app.add_middleware(SessionMiddleware, secret_key=settings.jwt_secret_key)

    http.Routes.load(app)

    @app.exception_handler(StarletteHTTPException)
    async def exception_handler(_request: Request, exc: HTTPException) -> Union[Response, Awaitable[Response]]:
        return http.ResponseBuilder.exception(exc)

    @app.exception_handler(status.HTTP_404_NOT_FOUND)
    async def not_found(_request: Request, exc: HTTPException) -> Union[Response, Awaitable[Response]]:
        exc.detail = exc.detail or 'Resource Not found'
        return http.ResponseBuilder.exception(exc)

    @app.exception_handler(status.HTTP_500_INTERNAL_SERVER_ERROR)
    async def internal_server_error(_request: Request, exc: RuntimeError) -> Union[Response, Awaitable[Response]]:
        e = HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=str(exc),
        )

        return http.ResponseBuilder.exception(e)

    return app


def create_app() -> FastAPI:
    app = FastAPI(
        title='Knight Quest API',
        # lifespan=lifespan,
        default_response_class=http.Response,
        responses={
            200: {
                'description': 'Successful response',
                'model': create_model(
                    'Success',
                    success=(bool, True),
                    message=(str, 'Success.'),
                    data=(dict, None)
                )
            },
            500: {
                'description': 'Internal server error',
                'model': create_model(
                    'InternalServerError',
                    success=(bool, False),
                    message=(str, 'Internal server error.')
                )
            }
        }
    )

    setup_app(app)

    return app
