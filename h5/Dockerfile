FROM nginx:alpine as production
WORKDIR /app
COPY ./dist/build/h5 /usr/share/nginx/html
COPY default.conf /etc/nginx/conf.d/

EXPOSE 80 443