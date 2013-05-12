NetPonto.Presencas
==================

Programa usado para gerir as presenças nos eventos da NetPonto.

Como correr a aplicaçao
-----------------------

É preciso configurar as chaves do Eventbrite (onde estão criados os eventos)
e a ligação a um servidor RavenDB (onde são guardados os documentos com as presenças):

- Chaves do eventbrite são configuradas no web.config, nas appSettings 'eventbrite.app_key' e 'eventbrite.user_key'.
- Ligaçao a servidor RavenDB configurada em connectionstrings.config.

Recomenda-se também que se altere o username e password que esta em 'App_Data\users.xml', que é usado para controlar o acesso à aplicação.

Como usar a aplicaçao
---------------------

A aplicaçao está feita a pensar no uso num tablet, que pode estar ou não permanentemente ligado à internet. 

Quando se inicia, deve-se carregar em 'refresh events data' para atualizar a lista de eventos. 
Quando se escolhe um evento, deve-se então carregar em 'refresh attendees data' para obter a lista de registos.
Após os registos, grava-se no servidor com 'save data remotely' e podem-se obter duas listas de participantes, uma de texto e uma em excel.

Os dados são guardados no 'localstorage' do browser, pelo que podem ser carregados antes do evento, registados os participantes, e posteriormente gravados no servidor.

Existe suporte muito básico para o uso de vários registos em simultâneo através de SignalR, mas a funcionalidade ainda não está à prova de bala.
