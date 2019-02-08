# IGTI-PA
PoC da solução desenvolvida para o Projeto Aplicado do IGTI

Contém as classes responsáveis por atender as requisições a cada Api do Portal do Cliente.

/Dynamics365Api: serviço responsável por buscar e gravar dados de contatos (pessoas), empresas (clientes), perfis de acesso dos contatos e linhas de serviço contratadas, além de abrir e listar chamados no CRM Dynamics 365;
/MicrosoftGraphApi: serviço responsável por listar e manter usuários de Office365, cadastrar grupos de e-mail e alterar senha de usuários;
/PortalClienteDBApi: serviço responsável por manter os dados da aplicação em banco de dados Azure SQL Server. Serão mantidos os termos de aceite cadastrados e os campos a serem exibidos para cadastro de usuários de cada cliente.

Cada uma dessas APIs possui vários endpoints para serem chamados pela aplicação front-end.
