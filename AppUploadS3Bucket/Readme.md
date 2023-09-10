Essa aplicação Console ao ser executada tem por objetivo criar/atualizar arquivos dentro do meu Bucket S3 cujo nome é sqsnotificacao. 

Esse Bucket já está configurado na AWS com acesso público. O caminho do diretório de onde será buscado os arquivos para serem inseridos/atualizados no bucket deve ser apontado no arquivo appSettings.json na chave de entrada "DirectoryPath".

As demais configurações desse arquivo appSettings.json devem permanecer inalteradas pois se referem as credenciais e ao nome do meu bucket devidamente configurado na AWS. 
