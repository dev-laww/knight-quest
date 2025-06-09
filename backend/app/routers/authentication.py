from fastapi import APIRouter, Request

from ..utils.auth_providers import oauth
from ..utils.http import ResponseBuilder

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
    print("TOKEN STRUCTURE:", token)
    print("TOKEN KEYS:", list(token.keys()))

    user_info = await oauth.google.userinfo(token=token)
    return ResponseBuilder.build_success('Authentication successful!', data=user_info)

router.include_router(google_router)
