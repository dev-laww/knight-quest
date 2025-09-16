from typing import Optional

from sqlalchemy import JSON, Column
from sqlmodel import SQLModel, Field, Relationship

from .user import User


class Save(SQLModel, table=True):
    __tablename__ = "saves"

    id: Optional[int] = Field(default=None, primary_key=True)
    name: str
    data: Optional[dict] = Field(sa_column=Column(JSON))
    user_id: int = Field(foreign_key="users.id", unique=True)

    user: Optional[User] = Relationship(back_populates="save")
