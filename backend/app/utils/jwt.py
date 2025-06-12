from datetime import UTC, datetime, timedelta
from typing import Optional

import jwt as pyjwt
from jwt.exceptions import InvalidTokenError
from pydantic import BaseModel

from ..settings import settings


class Token(BaseModel):
    access: str
    token_type: str


def decode(token: str) -> Optional[dict]:
    try:
        return pyjwt.decode(token, settings.jwt_secret_key, algorithms=['HS256'])
    except InvalidTokenError:
        return None


def encode(data: dict, expires_in: Optional[timedelta] = None) -> str:
    to_encode = data.copy()
    expire = datetime.now(tz=UTC) + expires_in if expires_in else datetime.now(tz=UTC) + timedelta(days=999)
    to_encode.update({"exp": expire})

    return pyjwt.encode(to_encode, settings.jwt_secret_key, algorithm='HS256')
