import { cleanEnv, num, str, testOnly } from 'envalid'
import * as process from 'node:process'
import 'dotenv/config';

export const env = cleanEnv(process.env, {
    NODE_ENV: str({ choices: [ 'development', 'production', 'test' ], default: 'development' }),
    PORT: num({ default: 8000 }),
    JWT_SECRET: str({ devDefault: testOnly('') }),
    DATABASE_URL: str()
});