FROM node:lts AS base
WORKDIR /app

ARG PUBLIC_HOST_URL
ARG PUBLIC_API_URL
ARG PUBLIC_ENVIRONMENT
ARG ASTRO_ADAPTER

ENV PUBLIC_HOST_URL=$PUBLIC_HOST_URL
ENV PUBLIC_API_URL=$PUBLIC_API_URL
ENV PUBLIC_ENVIRONMENT=$PUBLIC_ENVIRONMENT
ENV ASTRO_ADAPTER=$ASTRO_ADAPTER

COPY package*.json ./

RUN npm ci --audit=false --fund=false

FROM base AS build
COPY . .
RUN npm run build

FROM node:lts AS runtime
WORKDIR /app

COPY --from=base /app/node_modules ./node_modules
COPY --from=build /app/dist ./dist

ENV HOST=0.0.0.0
ENV PORT=4321

EXPOSE 4321
CMD ["node", "./dist/server/entry.mjs"]
