#!/bin/sh

# run before with path to a compiled build as well as robots and fields directory
# also allow user to specify version

show_help() {
	echo "-h	display help message"
	echo "-f	specify the input directory for fields"
	echo "-r	specify the input directory for robots"
	echo "-b	specify the build directory of synthesis"
}

OPTIND=1

APP_NAME="Synthesis"
INIT_DIR="$(dirname "$(readlink -f "${0}")")"
APP_DIR="$INIT_DIR/$APP_NAME.AppDir"

while getopts "h?f:r:b:" opt; do
	case "$opt" in
		h|\?)
			show_help
			exit 0
			;;
		f)
			fields="$OPTARG"
			;;
		r)
			robots="$OPTARG"
			;;
		b)
			build="$OPTARG"
			;;
	esac
done

shift $((OPTIND-1))

if [ ! -n "$fields" ] ; then
	echo "Specify input directory for fields using \"-f\""
	exit 1
fi


if [ ! -n "$robots" ] ; then
	echo "Specify input directory for robots using \"-r\""
	exit 1
fi

if [ ! -n "$build" ] ; then
	echo "Specify synthesis build directory using \"-b\""
	exit 1
fi

cp "$fields/"*.mira "$APP_DIR/fields/" 
cp "$robots/"*.mira "$APP_DIR/robots/"
cp -R "$build/"* "$APP_DIR/usr/bin" 
