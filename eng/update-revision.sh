#!/bin/sh
echo "export const Revision: string = \"$1\";" > "$(dirname "$0")/../ts/landing/src/revision.ts"