FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm

# Instalar Node.js LTS (v22) e Angular CLI
# Nota: removemos /etc/apt/sources.list.d/yarn.list para evitar erros de chaves GPG expiradas no repositório do Yarn
RUN rm -f /etc/apt/sources.list.d/yarn.list \
    && curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get update \
    && apt-get install -y nodejs \
    && npm install -g @angular/cli@latest

# Garantir que ferramentas do dotnet (como dotnet-ef) fiquem no PATH
ENV PATH="${PATH}:/home/vscode/.dotnet/tools"
