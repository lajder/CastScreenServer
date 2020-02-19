
# Command to run app without Admin privileges
    netsh http add urlacl url=http://+:80/ user=DOMAIN\user

# Start app with parameters
    CastScreenServer.exe $ipAddr $Port