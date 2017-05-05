#!/bin/sh

#ffmpeg=ffmpeg
#ffmpeg=/mnt/ext/opt/medialibrary/bin/ffmpeg
ffmpeg=/opt/bin/ffmpeg

for i in `find . -name "*.[Mm][Tt][Ss]" | grep -v thumb | grep -v "^\."`; do
  j=`basename $i .mts`
  j=`basename $j .MTS`
  k=`dirname $i`
  n=$k/MV_${j}.mp4
  if [ -f $n ]; then
    continue;
  fi
  echo $i to $n...
  # ffmpeg -y -i $i -f mp4 -vcodec copy -acodec libvo_aacenc -ab 192k -ar 44100 -ac 2 $n
  $ffmpeg -n -i $i -f mp4 -vcodec libx264 -profile:v baseline -aspect 16:9 -preset:v superfast -level:v 3.2 -acodec ac3 -ac 2 $n
done
