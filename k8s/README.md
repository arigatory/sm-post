# Kubernetes Deployment для CQRS Event Sourcing Application

## Описание архитектуры

Приложение состоит из следующих компонентов:

### Stateful сервисы (1 реплика):
- **postgres-marten** - PostgreSQL для event store (Marten)
- **postgres-read** - PostgreSQL для read database (Query side)
- **kafka** - Kafka в режиме KRaft (без Zookeeper)

### Stateless сервисы (2 реплики для отказоустойчивости):
- **post-cmd-api** - Command API (порт 30262)
- **post-query-api** - Query API (порт 30263)
- **client** - Next.js frontend (порт 30000)

### Вспомогательные сервисы:
- **kafka-ui** - Web UI для мониторинга Kafka (порт 30080)

## Предварительные требования

1. Локальный Kubernetes кластер:
   - Docker Desktop with Kubernetes enabled
   - или Minikube: `minikube start`

2. Собранные Docker образы:
```bash
cd /Users/ivan/Learn/Udemy/CqrsEventSourcing/sm-post
docker-compose -f Infra/docker-compose.yml build
```

3. Для Minikube - загрузить образы в кластер:
```bash
minikube image load infra-post-cmd-api:latest
minikube image load infra-post-query-api:latest
minikube image load infra-client:latest
```

## Развертывание

### 1. Создание namespace и базовых сервисов
```bash
cd k8s

# Создать namespace
kubectl apply -f namespace.yaml

# Развернуть PostgreSQL
kubectl apply -f postgres-marten.yaml
kubectl apply -f postgres-read.yaml

# Развернуть Kafka
kubectl apply -f kafka.yaml
kubectl apply -f kafka-ui.yaml
```

### 2. Ожидание готовности инфраструктуры
```bash
# Проверить статус
kubectl get pods -n cqrs-app -w

# Дождаться пока все поды будут Running
# Должно быть:
# - postgres-marten-0: Running
# - postgres-read-0: Running
# - kafka-0: Running
# - kafka-ui-xxx: Running
```

### 3. Развертывание приложений (с 2 репликами каждое)
```bash
# Развернуть Command API (2 реплики)
kubectl apply -f post-cmd-api.yaml

# Развернуть Query API (2 реплики)
kubectl apply -f post-query-api.yaml

# Развернуть Client (2 реплики)
kubectl apply -f client.yaml
```

### 4. Проверка развертывания
```bash
# Проверить все поды
kubectl get pods -n cqrs-app

# Должно быть:
# postgres-marten-0       1/1  Running
# postgres-read-0         1/1  Running
# kafka-0                 1/1  Running
# kafka-ui-xxx            1/1  Running
# post-cmd-api-xxx-1      1/1  Running
# post-cmd-api-xxx-2      1/1  Running
# post-query-api-xxx-1    1/1  Running
# post-query-api-xxx-2    1/1  Running
# client-xxx-1            1/1  Running
# client-xxx-2            1/1  Running

# Проверить сервисы
kubectl get svc -n cqrs-app

# Проверить логи
kubectl logs -n cqrs-app deployment/post-cmd-api --tail=50
kubectl logs -n cqrs-app deployment/post-query-api --tail=50
```

## Доступ к приложению

### Docker Desktop:
- **Frontend**: http://localhost:30000
- **Command API**: http://localhost:30262
- **Query API**: http://localhost:30263
- **Kafka UI**: http://localhost:30080

### Minikube:
```bash
# Получить IP minikube
minikube ip

# Доступ по адресу:
# http://<minikube-ip>:30000 - Frontend
# http://<minikube-ip>:30262 - Command API
# http://<minikube-ip>:30263 - Query API
# http://<minikube-ip>:30080 - Kafka UI
```

## Масштабирование

### Увеличить количество реплик:
```bash
# Увеличить Command API до 3 реплик
kubectl scale deployment post-cmd-api -n cqrs-app --replicas=3

# Увеличить Query API до 3 реплик
kubectl scale deployment post-query-api -n cqrs-app --replicas=3

# Увеличить Client до 3 реплик
kubectl scale deployment client -n cqrs-app --replicas=3
```

### Автомасштабирование (HPA):
```bash
# Включить автомасштабирование для Command API
kubectl autoscale deployment post-cmd-api -n cqrs-app --min=2 --max=5 --cpu-percent=80

# Включить автомасштабирование для Query API
kubectl autoscale deployment post-query-api -n cqrs-app --min=2 --max=5 --cpu-percent=80
```

## Отказоустойчивость

### Проверка отказоустойчивости Command API:
```bash
# Получить список подов
kubectl get pods -n cqrs-app -l app=post-cmd-api

# Удалить один под
kubectl delete pod -n cqrs-app <pod-name>

# Kubernetes автоматически создаст новый под
# Второй под продолжит обрабатывать запросы
kubectl get pods -n cqrs-app -l app=post-cmd-api -w
```

### Проверка балансировки нагрузки:
```bash
# Проверить endpoints
kubectl get endpoints -n cqrs-app

# Должно быть 2 IP для post-cmd-api, post-query-api и client
```

## Мониторинг и отладка

### Логи:
```bash
# Логи всех реплик Command API
kubectl logs -n cqrs-app -l app=post-cmd-api --tail=100 -f

# Логи конкретного пода
kubectl logs -n cqrs-app <pod-name> --tail=100 -f

# Логи предыдущего запуска пода (если был рестарт)
kubectl logs -n cqrs-app <pod-name> --previous
```

### Описание ресурсов:
```bash
# Детальная информация о deployment
kubectl describe deployment post-cmd-api -n cqrs-app

# Информация о поде
kubectl describe pod <pod-name> -n cqrs-app

# События в namespace
kubectl get events -n cqrs-app --sort-by='.lastTimestamp'
```

### Подключение к контейнеру:
```bash
# Shell в контейнер
kubectl exec -it -n cqrs-app <pod-name> -- /bin/sh

# Для PostgreSQL
kubectl exec -it -n cqrs-app postgres-marten-0 -- psql -U postgres -d eventstore
kubectl exec -it -n cqrs-app postgres-read-0 -- psql -U postgres
```

## Удаление

### Удалить приложения:
```bash
kubectl delete -f client.yaml
kubectl delete -f post-query-api.yaml
kubectl delete -f post-cmd-api.yaml
kubectl delete -f kafka-ui.yaml
kubectl delete -f kafka.yaml
kubectl delete -f postgres-read.yaml
kubectl delete -f postgres-marten.yaml
```

### Удалить namespace (со всеми ресурсами):
```bash
kubectl delete namespace cqrs-app
```

### Удалить PersistentVolumes (если нужно):
```bash
kubectl get pv | grep cqrs-app
kubectl delete pv <pv-name>
```

## Troubleshooting

### Проблема: Поды не запускаются
```bash
# Проверить события
kubectl get events -n cqrs-app --sort-by='.lastTimestamp'

# Проверить описание пода
kubectl describe pod <pod-name> -n cqrs-app

# Частые причины:
# - Образ не найден: убедитесь что образы собраны и загружены в minikube
# - Недостаточно ресурсов: проверьте ресурсы кластера
```

### Проблема: ImagePullBackOff
```bash
# Для Docker Desktop - проверить что образы существуют
docker images | grep infra

# Для Minikube - загрузить образы
minikube image load infra-post-cmd-api:latest
minikube image load infra-post-query-api:latest
minikube image load infra-client:latest

# Проверить загруженные образы
minikube image ls | grep infra
```

### Проблема: CrashLoopBackOff
```bash
# Проверить логи
kubectl logs -n cqrs-app <pod-name>

# Проверить предыдущие логи
kubectl logs -n cqrs-app <pod-name> --previous

# Частые причины:
# - База данных не готова: подождите пока PostgreSQL/Kafka запустятся
# - Неправильная конфигурация: проверьте ConfigMap
```

### Проблема: Service не доступен
```bash
# Проверить endpoints
kubectl get endpoints -n cqrs-app

# Проверить что поды Running и Ready
kubectl get pods -n cqrs-app

# Проверить labels
kubectl get pods -n cqrs-app --show-labels
```

## Производственные улучшения

Для production окружения рекомендуется добавить:

1. **Ingress Controller** для HTTPS и доменов
2. **cert-manager** для автоматических SSL сертификатов
3. **External Secrets Operator** для управления секретами
4. **Prometheus + Grafana** для мониторинга
5. **EFK/ELK Stack** для централизованных логов
6. **Service Mesh** (Istio/Linkerd) для продвинутой балансировки
7. **NetworkPolicies** для сетевой безопасности
8. **PodDisruptionBudgets** для контроля обновлений
9. **ResourceQuotas** для ограничения ресурсов
10. **Backup решения** для StatefulSet (Velero)
