FROM openjdk:8

ENV JAVA_OPTS

ADD . /server
WORKDIR /server

RUN echo "eula=true" > /server/eula.txt

CMD java $JAVA_OPTS minecraft_server.jar nogui