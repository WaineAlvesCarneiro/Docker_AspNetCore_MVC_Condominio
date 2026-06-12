# Limpar Docker completamente
cd D:\Projetos\Docker_AspNetCore_MVC_Condominio\

docker compose down -v
docker images -aq | % { docker rmi -f $_ }
docker volume ls -q | % { docker volume rm $_ }
docker network prune -f
docker builder prune -a -f
docker buildx prune -f
docker system prune -a -f

# Extra: remover builders criados manualmente
docker buildx ls | ForEach-Object {
    $builder = ($_ -split '\s+')[0]
    if ($builder -ne "default") {
        docker buildx rm $builder
    }
}

docker images -aq | % { docker rmi -f $_ }

docker buildx rm default
docker buildx create --use

docker ps -aq | % { docker rm -f $_ }