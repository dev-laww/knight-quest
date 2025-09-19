from typing import TypeVar, Type, Optional, List

from pydantic import BaseModel
from sqlalchemy import func
from sqlalchemy.ext.asyncio import AsyncSession
from sqlmodel import select

T = TypeVar('T', bound=BaseModel)


class Repository[T]:
    """Base repository class for CRUD operations."""

    def __init__(self, session: AsyncSession, model: Type[T]):
        self.__session = session
        self.__model = model

    @property
    def session(self) -> AsyncSession:
        return self.__session

    @property
    def model(self) -> Type[T]:
        return self.__model

    async def get(self, id: int) -> Optional[T]:
        query = select(self.model).where(self.model.id == id)  # noqa
        result = await self.session.execute(query)
        return result.scalars().first()

    async def all(self, **filters) -> List[T]:
        query = select(self.model).filter_by(**filters)
        result = await self.session.execute(query)
        return result.scalars().all()  # noqa

    async def create(self, data: T) -> T:
        obj = self.model.model_validate(data)

        self.session.add(obj)

        await self.session.commit()
        await self.session.refresh(obj)
        return obj

    async def update(self, id: int, data: T) -> Optional[T]:
        obj = await self.get(id)
        if not obj:
            return None

        for key, value in data.model_dump(exclude_unset=True).items():
            setattr(obj, key, value)

        self.session.add(obj)

        await self.session.commit()
        await self.session.refresh(obj)
        return obj

    async def delete(self, id: int) -> bool:
        obj = await self.get(id)
        if not obj:
            return False

        await self.session.delete(obj)
        await self.session.commit()
        return True

    async def count(self, **filters) -> int:
        query = select(func.count(self.model)).filter_by(**filters)
        result = await self.session.execute(query)
        return result.scalar() or 0
