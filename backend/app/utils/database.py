from sqlalchemy.ext.asyncio import create_async_engine, AsyncSession
from sqlmodel import create_engine, Session

from app.settings import settings

async_engine = create_async_engine(settings.database_url)
engine = create_engine(settings.database_url.replace('asyncpg', 'psycopg2'), echo=True)


async def get_async_session():
    """Get an asynchronous database session."""
    async with AsyncSession(async_engine) as session:
        yield session


def get_session():
    """Get a synchronous database session."""
    with Session(engine) as session:
        yield session
