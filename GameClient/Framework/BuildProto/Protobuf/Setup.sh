#!/bin/bash
_ROOT_PATH=$(cd `dirname $0`; pwd)
cd ${_ROOT_PATH}

cd protobuf-3.14.0/

./autogen.sh 
./configure
make
make install

cd python

python3 setup.py build
python3 setup.py install
python3 setup.py test
sleep 30s
