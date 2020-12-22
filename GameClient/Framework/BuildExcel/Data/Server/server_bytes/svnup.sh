#!/bin/sh

svn up --accept 'theirs-conflict'
## @ADD derrick
exit 0;

revision=`svn info |grep "后修改的版本:" |awk '{print $2}'`
echo "the revision is $revision"
URL=`svn info |grep URL: |awk '{print $2}'`
echo "the url is $URL"
svnRevision=`svn info $URL |grep "后修改的版本:" |awk '{print $2}'`
echo "the revision in svn is $svnRevision"
if [[ $revision < $svnRevision ]] ; then
    echo -e "\033[0;31;40m need svn up \033[0m"
    svn up --accept 'theirs-conflict'
    echo "svn up finish"
    exit 1
else
    echo -e "\033[0;38;40m need svn up \033[0m"
    exit 0
fi
