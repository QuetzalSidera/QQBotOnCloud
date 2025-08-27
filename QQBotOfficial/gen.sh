#!/bin/bash

# ============ 配置 ===============
CERT_NAME="mycert"
PFX_FILE="mycert.pfx"
PASSWORD="yourpassword"
DAYS=3650   # 有效期 10 年
# =================================

echo ">> 生成 RSA 私钥..."
openssl genrsa -out $CERT_NAME.key 2048

echo ">> 生成自签名证书 (X.509)..."
openssl req -new -x509 -key $CERT_NAME.key -out $CERT_NAME.crt -days $DAYS -subj "/CN=localhost"

echo ">> 将证书和私钥打包为 PFX..."
openssl pkcs12 -export -out $PFX_FILE -inkey $CERT_NAME.key -in $CERT_NAME.crt -password pass:$PASSWORD

echo ">> 生成完成: $PFX_FILE"
echo "   密码: $PASSWORD"
