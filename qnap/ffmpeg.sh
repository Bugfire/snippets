#!/bin/bash

#ffmpeg=ffmpeg
#ffmpeg=/mnt/ext/opt/medialibrary/bin/ffmpeg
ffmpeg=/opt/bin/ffmpeg

for i in `find . -name "*.[Mm][Tt][Ss]" | grep -v thumb | grep -v "/\."`; do
  j=`basename $i .mts`
  j=`basename $j .MTS`
  k=`dirname $i`
  n=$k/MV_${j}.mp4
  if [ -f $n ]; then
    continue;
  fi
  echo $i to $n...
  $ffmpeg -y -i $i -f mp4 -vcodec libx264 -profile:v baseline -preset:v superfast -level:v 3.2 -acodec ac3 -ac 2 $n.tmp
  mv $n.tmp $n
done
