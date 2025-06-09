import enum

from pydantic_settings import BaseSettings, SettingsConfigDict


class AppMode(enum.Enum):
    DEVELOPMENT = 'development'
    PRODUCTION = 'production'


class Settings(BaseSettings):
    app_mode: AppMode = AppMode.DEVELOPMENT

    model_config = SettingsConfigDict(
        env_file='.env',
        env_file_encoding='utf-8',
        env_ignore_empty=True
    )


settings = Settings()
