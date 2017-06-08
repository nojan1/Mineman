FROM openjdk:8

ENV JAVA_OPTS "-Xmx1024m -Xms512m"

ADD . /server
WORKDIR /server

RUN echo "eula=true" > /server/eula.txt

CMD if [ -f "COMMAND" ]; then command="$(cat 'COMMAND')"; else command="minecraft_server.jar"; fi ; exec java $JAVA_OPTS -jar $command nogui