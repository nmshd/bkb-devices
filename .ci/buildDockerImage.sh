#!/bin/bash
set -e
set -u
set -x

docker build --file ./Devices.API/Dockerfile --tag ghcr.io/nmshd/bkb-devices:${TAG-temp} .
