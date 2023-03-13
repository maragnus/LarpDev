#!/bin/sh
echo "namespace Larp.Landing.Shared;

public class Revision
{
    public const string BuildRevision = \"$1\";
}" > "$(dirname "$0")/../cs/Larp/Larp.Landing/Shared/Revision.cs"