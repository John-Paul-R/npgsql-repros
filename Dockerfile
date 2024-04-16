# Use the official PostgreSQL image as the base image
FROM postgres:16

# Set the environment variables for the PostgreSQL database
ENV POSTGRES_DB=npgsql_repros
ENV POSTGRES_USER=postgres
ENV POSTGRES_HOST_AUTH_METHOD=trust

# Expose the default PostgreSQL port
EXPOSE 5432
