
# Knight Quest API

This FastAPI project serves as a lightweight backend for a Knight Quest, handling basic authentication and simple database operations. It provides endpoints for user login, registration, and profile retrieval, as well as storing and accessing minimal game-related data.


## Authors

- [@dev-laww](https://www.github.com/dev-laww)


## Deployment

To deploy this project run

```bash
  npm run deploy
```


## Environment Variables

To run this project, you will need to add the following environment variables to your .env file

`GOOGLE_CLIENT_SECRET`

`GOOGLE_CLIENT_ID`

`JWT_SECRET_KEY`

or simply run this command and edit the variables

```bash
cp ./.env.example .env
```

**Note: For more information on getting a client secret and id you can visit this [link](https://developers.google.com/identity/gsi/web/guides/get-google-api-clientid)*


## Getting Started

1. Clone the repository
2. Install the dependencies
    ```bash
    poetry install
    ```
3. Run the application
    ```bash
    poetry run fastapi dev main.py # for development
    # or
    poetry run fastapi run main.py # for production
    ```
