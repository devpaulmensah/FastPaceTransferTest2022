# Fast Pace Transfer Test

```bash
# restore dependencies
$ dotnet restore

# setup email
update configuration with your email and password to send otp code

# install redis using docker with the following commands
$ docker pull redis
$ docker run --name redis -d redis

# run project
start redis in docker and run app
$ dotnet run - start app
```
