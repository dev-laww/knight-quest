from enum import Enum
from typing import List, Optional

from sqlalchemy import Column, String
from sqlmodel import SQLModel, Field, Relationship


class UserRole(str, Enum):
    teacher = 'teacher'
    parent = 'parent'
    student = 'student'


class TeacherStudentLink(SQLModel, table=True):
    __tablename__ = 'teacher_student_link'

    teacher_id: Optional[int] = Field(default=None, foreign_key='users.id', primary_key=True)
    student_id: Optional[int] = Field(default=None, foreign_key='users.id', primary_key=True)


class ParentStudentLink(SQLModel, table=True):
    __tablename__ = 'parent_student_link'

    parent_id: Optional[int] = Field(default=None, foreign_key='users.id', primary_key=True)
    student_id: Optional[int] = Field(default=None, foreign_key='users.id', primary_key=True)


class User(SQLModel, table=True):
    __tablename__ = 'users'

    id: Optional[int] = Field(default=None, primary_key=True)
    email: str = Field(sa_column=Column(String, index=True, nullable=False, unique=True))
    password_hash: str = Field(nullable=False)
    first_name: str = Field(nullable=False)
    last_name: str = Field(nullable=False)

    role: UserRole = Field(sa_column=Column(String, nullable=False, default=UserRole.student))

    # Teacher-Student relationships
    students: List['User'] = Relationship(
        back_populates='teachers',
        link_model=TeacherStudentLink,
        sa_relationship_kwargs={
            'primaryjoin': 'User.id == TeacherStudentLink.teacher_id',
            'secondaryjoin': 'User.id == TeacherStudentLink.student_id',
        }
    )

    teachers: List['User'] = Relationship(
        back_populates='students',
        link_model=TeacherStudentLink,
        sa_relationship_kwargs={
            'primaryjoin': 'User.id == TeacherStudentLink.student_id',
            'secondaryjoin': 'User.id == TeacherStudentLink.teacher_id',
        }
    )

    # Parent-Student relationships
    children: List['User'] = Relationship(
        back_populates='parents',
        link_model=ParentStudentLink,
        sa_relationship_kwargs={
            'primaryjoin': 'User.id == ParentStudentLink.parent_id',
            'secondaryjoin': 'User.id == ParentStudentLink.student_id',
        }
    )

    parents: List['User'] = Relationship(
        back_populates='children',
        link_model=ParentStudentLink,
        sa_relationship_kwargs={
            'primaryjoin': 'User.id == ParentStudentLink.student_id',
            'secondaryjoin': 'User.id == ParentStudentLink.parent_id',
        }
    )

    save: Optional['Save'] = Relationship(back_populates='user')

    def verify_password(self, password: str) -> bool:
        return self.password_hash == password  # TODO: update password verification logic to use hash
