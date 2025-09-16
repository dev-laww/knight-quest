from fastapi import APIRouter, Request
from starlette.responses import RedirectResponse, JSONResponse

from ..utils.auth_providers import oauth
from ..utils.jwt import *

router = APIRouter(
    prefix='/auth',
    tags=['Authentication']
)

google_router = APIRouter(prefix='/google')


@google_router.get("/login")
async def login(request: Request):
    redirect_uri = request.url_for("auth_callback")
    return await oauth.google.authorize_redirect(request, redirect_uri)


@google_router.get("/callback")
async def auth_callback(request: Request):
    token = await oauth.google.authorize_access_token(request)

    user_info = await oauth.google.userinfo(token=token)
    token = encode(user_info)

    return {
        'token': token,
        'user': user_info
    }


router.include_router(google_router)
