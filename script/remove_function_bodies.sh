#!/bin/bash
IFS= read -r -d '' variable

TMP=$(echo "$variable" \
| dos2unix \
| tr '\n' '\r' \
| perl -pe 's/[\w ]*class\s+[\w ]*?(\w+)\s*{/class \1 {\nprivate:\n/g' \
| perl -pe 's/[\w ]+\([^\)]*\)[\s\w]*\K\{((?:\{(?-1)\}|[^{}]++)*)\}/;/g' \
| perl -pe 's/\([^)]*[^;]\K\n+\s*//g' \
| perl -pe 's/(,\n*\s+)/, /g' \
| tr '\r' '\n' \
| unix2dos \
| sed -e '/^\s*$/d' -e '/^\s*\/\/.*$/d' -e '/^GENERATEADAPTOR$/d' -e 's/)\s*;/);/g' \
)
echo "$TMP"