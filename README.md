# IGTI-PA
PoC da solução desenvolvida para o Projeto Aplicado do IGTI

Contém as classes responsáveis por atender as requisições a cada Api do Portal do Cliente.

<b>Azure Function:</b> função serverless destinada a realizar um processamento que envolve requisições síncronas a mais de uma API RESTful disponibilizada. Esta function será disparada via webhook (http) através da aplicação front-end, sempre que for necessário recuperar dados das três fontes distintas (Azure SQL Server, Dynamics365 e MS Graph), e irá retornar os dados do usuário logado na aplicação naquele momento, como seus perfis de acesso, contatos, direitos, linhas de serviço, etc; 

<b>Dynamics365Api:</b> serviço responsável por buscar e gravar dados de contatos (pessoas), empresas (clientes), perfis de acesso dos contatos e linhas de serviço contratadas, além de abrir e listar chamados no CRM Dynamics 365; 

<b>MicrosoftGraphApi:</b> serviço responsável por listar e manter usuários de Office365, cadastrar grupos de e-mail e alterar senha de usuários;

<b>PortalClienteDBApi:</b> serviço responsável por manter os dados da aplicação em banco de dados Azure SQL Server. Serão mantidos os termos de aceite cadastrados e os campos a serem exibidos para cadastro de usuários de cada cliente.

Cada uma dessas APIs possui vários endpoints para serem chamados pela aplicação front-end.
