from fastcrud import crud_router

from ..models import Save
from ..utils.database import get_async_session

router = crud_router(
    session=get_async_session,
    model=Save,
    create_schema=Save,
    update_schema=Save,
    path='/saves',
    tags=['Saves']
)

# TODO: Implement custom endpoints for saves