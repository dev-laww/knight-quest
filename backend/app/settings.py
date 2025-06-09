import enum

from pydantic_settings import BaseSettings, SettingsConfigDict


class AppMode(enum.Enum):
    DEVELOPMENT = 'development'
    PRODUCTION = 'production'


class Settings(BaseSettings):
    google_client_id: str
    google_client_secret: str
    jwt_secret_key: str
    app_mode: AppMode = AppMode.DEVELOPMENT

    model_config = SettingsConfigDict(
        env_file='.env',
        env_file_encoding='utf-8',
        env_ignore_empty=True
    )


settings = Settings()
