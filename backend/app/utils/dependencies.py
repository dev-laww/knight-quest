from typing import Type, TypeVar, Callable

from fastapi import Depends
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from .database import get_async_session
from ..repositories import Repository

T = TypeVar('T', bound=BaseModel)


def create_repository_dependency(model_class: Type[T]) -> Callable[[], Repository[T]]:
    def get_repo(session: AsyncSession = Depends(get_async_session)) -> Repository[T]:
        return Repository(session, model_class)

    return get_repo


def get_repository_dependency(repo_class: Type[Repository[T]]) -> Callable[[], Repository[T]]:
    """Get a dependency function for a specific repository class."""

    def get_repo(session: AsyncSession = Depends(get_async_session)) -> Repository[T]:
        # repository classes have their model defined
        return repo_class(session)  # noqa

    return get_repo
