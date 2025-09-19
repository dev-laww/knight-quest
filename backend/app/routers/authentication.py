from fastapi import APIRouter, Request
from fastapi.params import Depends
from fastapi_utils.cbv import cbv

from ..models import User
from ..repositories import Repository
from ..schemas.auth import LoginSchema
from ..schemas.response import LoginResponse
from ..utils.auth_providers import oauth
from ..utils.dependencies import create_repository_dependency
from ..utils.jwt import *

router = APIRouter(
    prefix='/auth',
    tags=['Authentication']
)

google_router = APIRouter(prefix='/google')


@cbv(router)
class Auth:
    repository: Repository[User] = Depends(create_repository_dependency(User))

    @router.post('/login', response_model=LoginResponse)
    async def login(self, login_data: LoginSchema):
        user = await self.repository.all(email=login_data.email)
        user = user[0] if user else None

        if not user or not user.verify_password(login_data.password):
            return {'error': 'Invalid email or password'}

        token = encode({'user_id': user.id, 'email': user.email})

        return {
            'token': token,
            'user': user.model_dump(exclude={'password_hash'})
        }


@cbv(google_router)
class GoogleAuth:
    @google_router.get('/login', response_model=LoginResponse)
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
