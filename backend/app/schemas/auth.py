from .base import Base


class LoginSchema(Base):
    email: str
    password: str
