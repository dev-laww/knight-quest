from fastcrud import crud_router

from ..models import User
from ..utils.database import get_async_session

router = crud_router(
    session=get_async_session,
    model=User,
    create_schema=User,
    update_schema=User,
    path='/users',
    tags=['users']
)

# TODO: Implement custom endpoints for users
