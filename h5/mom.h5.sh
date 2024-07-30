rm -f mom.h5.yml
cp mom.h5-template.yml mom.h5.yml
sed -i s/{tag}/$1/g ./mom.h5.yml
