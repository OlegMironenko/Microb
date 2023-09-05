# Microb

DRAFT

To build all project images you should run next command from src folder

dotnet publish --os linux --arch x64 /t:PublishContainer -c Release

To allow access from container to database add next line to pg_hba.conf

host    all    			all             172.0.0.1/8             trust