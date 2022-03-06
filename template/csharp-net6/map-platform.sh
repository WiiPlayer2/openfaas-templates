case $TARGETARCH in
    amd64)
        echo "linux-x64"
        ;;

    *)
        echo "linux-$TARGETARCH"
        ;;
esac
