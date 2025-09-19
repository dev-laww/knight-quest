from fastapi import APIRouter, Request
from fastapi_utils.cbv import cbv

from ..utils.auth_providers import oauth
from ..utils.jwt import *

router = APIRouter(
    prefix='/auth',
    tags=['Authentication']
)

google_router = APIRouter(prefix='/google')


@cbv(router)
class Auth:



@cbv(google_router)
class GoogleAuth:
    @google_router.get('/login')
    async def login(self, request: Request):
        base_url = str(request.base_url).rstrip('/')
        redirect_uri = f'{base_url}/auth/google/callback'
        return await oauth.google.authorize_redirect(request, redirect_uri)

    @google_router.get('/callback', name='auth_callback')
    async def auth_callback(self, request: Request):
        token = await oauth.google.authorize_access_token(request)

        user_info = await oauth.google.userinfo(token=token)
        token = encode(user_info)

        return {
            'token': token,
            'user': user_info
        }


router.include_router(google_router)
