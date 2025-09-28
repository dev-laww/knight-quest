import { PrismaClient } from '@prisma/client'
import { PrismaPg } from '@prisma/adapter-pg'
import { pagination } from 'prisma-extension-pagination'


const adapter = new PrismaPg({ connectionString: process.env.DATABASE_URL })
const client = new PrismaClient({ adapter })
    .$extends(pagination())

type ExtendedPrismaClient = typeof client

const globalForPrisma = global as unknown as { prisma: ExtendedPrismaClient }
export const prisma = globalForPrisma.prisma || client

if (process.env.NODE_ENV !== 'production') globalForPrisma.prisma = prisma