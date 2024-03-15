# Stage 1: Build the Angular app
FROM node:20.11.1-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build

# Stage 2: Serve the built app using Nginx
FROM nginx:1.21.0-alpine

COPY --from=build /app/dist/photo-showdown-frontend /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]