# Misc

## Icon

Using https://favicon.io/favicon-generator/

- M
- Rounded
- ABeeZee
- Regular 400 Normal
- 110
- #FFFFFF
- #209CEE

## Certificates

```bash
# Generate Certificate Authority (CA)
openssl req -x509 -sha256 -days 3650 -nodes -newkey rsa:2048 -subj "/CN=demos" -keyout ca.key -out ca.crt

# Create server Certificate Signing Request (CSR) configuration file
cat > server.conf <<EOF
[ req ]
default_bits = 2048
prompt = no
default_md = sha256
distinguished_name = dn

[ dn ]
CN = server

EOF

# Generate server private key
openssl genrsa -out server.key 2048

# Generate Certificate Signing Request (CSR) using server private key and configuration file
openssl req -new -key server.key -out server.csr -config server.conf

# Generate certificate with self signed ca
openssl x509 -req -in server.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out server.crt -days 3650 -sha256
```
