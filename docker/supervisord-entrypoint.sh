#!/bin/sh

ulimit -c 0

/usr/bin/supervisord -c /etc/supervisord.conf