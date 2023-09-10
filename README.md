# TesteS3SQS

Antes de executar essa aplicação favor entrar na pasta acima AppUploadS3Bucket e ler as orientações do Readme.md deste diretório, a aplicação dessa pasta AppUploadS3Bucket é um pré-requisito e deve ser executada antes de executar os passos informados abaixo.

Na raiz do projeto existe o arquivo Dockerfile. 

Para construir e executar o container, rode os comandos abaixo no diretório aonde o arquivo Dockerfile se encontra:

docker build -t appreadqueuesqs . para construir a imagem.
docker run appreadqueuesqs para rodar o container.

Essa aplicação do tipo Console tem por objetivo ler as notificações da minha queue MyQueueS3 configurada dentro do serviço SQS na AWS. Essa queue possui a seguinte policy abaixo (apenas para caráter informativo) configurada para receber notificações quando houver alguma atualização no meu bucket S3 chamado sqsnotificacao: 
{
  "Version": "2012-10-17",
  "Id": "__default_policy_ID",
  "Statement": [
    {
      "Sid": "__owner_statement",
      "Effect": "Allow",
      "Principal": {
        "AWS": "arn:aws:iam::070802546798:root",
        "Service": "s3.amazonaws.com"
      },
      "Action": "SQS:*",
      "Resource": "arn:aws:sqs:us-east-2:070802546798:MyQueueS3",
      "Condition": {
        "ArnLike": {
          "aws:SourceArn": "arn:aws:s3:::sqsnotificacao"
        }
      }
    }
  ]
}

Além disso outras configurações também existem no arquivo appSettings.json dessa aplicação (string de conexão com minha base MySQL, credenciais AWS de acesso ao bucket, e url da queue).

Caso queira validar também as informações em banco, minha instância MySQL está rodando num serviço RDS da AWS e está configurado com acesso público e sem restrições de ip. As informações de conexão com esta instância se encontra na chave "connectionString" do arquivo appSettings.json (No momento aqui não há preocupação com segurança apenas por se tratar de um teste)

O fluxo dessa aplicação Console conforme pode ser visto no código dela consiste nas seguintes etapas: 1) Receber as mensagens da queue, 2) Fazer as inserções/atualizações na base de dados MySql com as informações pedidas no documento de teste, 3) caso o registro não seja atualizado será gerado um arquivo de log no diretório raiz da aplicação com as devidas informações sobre o registro não atualizado e 4) por fim a mensagem é removida da queue.







