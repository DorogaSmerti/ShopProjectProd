# Code Review — MyFirstProject

**Дата:** 5 февраля 2025  
**Версия проекта:** текущее состояние

---

## Общая оценка: Intermediate

### Сводка по модулям

| Модуль          | Оценка | Комментарий                          |
|-----------------|--------|--------------------------------------|
| Архитектура     | 4/5    | Repository + Unit of Work, DI        |
| OrderService    | 2.5/5  | Баги, дублирование логики            |
| CartService     | 3/5    | Возможный баг с поиском по productId |
| ProductService  | 3.5/5  | Лишнее присвоение Id при Patch       |
| Контроллеры     | 3.5/5  | Нет await, повторяющийся код         |
| Безопасность    | 4/5    | JWT, Identity, нет валидации DTO     |
| Качество кода   | 3/5    | Смешение языков, опечатки            |

---

## Критичные баги

### 1. OrderController — отсутствует `await`

```csharp
// Строка 29 — GetOrderAsync возвращает Task, но await отсутствует
var order = _orderService.GetOrderAsync(userId, id);
```

**Проблема:** Возвращается `Task<OrderDto?>`, а не `OrderDto?`. Контроллер возвращает `Ok(task)`, а не `Ok(order)`.

**Исправление:**
```csharp
var order = await _orderService.GetOrderAsync(userId, id);
```

---

### 2. OrderService — бессмысленная проверка в цикле

```csharp
// Строки 52–56
foreach (var item in cartItems)
{
    if (cartItems == null)  // cartItems — список, по которому идёт цикл, null здесь невозможен
    {
        await _unitOfWork.RollbackTransactionAsync();
        return (OrderResult.ItemNotFound, null);
    }
```

**Исправление:** Проверять корзину до цикла:

```csharp
if (cartItems == null || cartItems.Count == 0)
{
    await _unitOfWork.RollbackTransactionAsync();
    return (OrderResult.ItemNotFound, null);
}
foreach (var item in cartItems) { ... }
```

---

### 3. OrderService — неправильные Id в OrderDto

```csharp
// Строки 104–109 — в OrderItemDto попадает item.Id от CartItem, а не от OrderItem
OrderItems = cartItems.Select(item => new OrderItemDto
{
    Id = item.Id,  // CartItem.Id, не OrderItem.Id
    ...
}).ToList()
```

**Исправление:** Использовать `OrderItems` заказа после сохранения или строить DTO из `newOrder.OrderItems`.

---

### 4. CartService — поиск по неверному полю

```csharp
// Строка 45 — передаётся productId
var existingItem = await _unitOfWork.CartItem.GetCartItemAsync(userId, productId);
```

В `ICartRepository` и `CartRepository` второй параметр — `cartItemId` (Id записи в корзине), а не `productId`:

```csharp
// CartRepository — ищет по p.Id == cartItemId
FirstOrDefaultAsync(p => p.UserId == userId && p.Id == cartItemId);
```

**Проблема:** Поиск идёт по PK корзины, а не по `ProductId`. Повторное добавление одного и того же товара создаёт дубликаты вместо обновления количества.

**Исправление:** Добавить `GetCartItemByProductAsync(userId, productId)` и использовать его при добавлении в корзину.

---

### 5. ProductService — изменение Id сущности при Patch

```csharp
// Строка 92
product.Id = newProductDto.Id;
```

**Проблема:** Менять Id существующей сущности недопустимо, возможны конфликты и нарушение целостности данных.

**Исправление:** Убрать присвоение `product.Id`.

---

## Проблемы среднего приоритета

### 6. OrderService — проглатывание исключений

```csharp
catch (Exception)
{
    await _unitOfWork.RollbackTransactionAsync();
    return (OrderResult.BadRequest, null);
}
```

Исключение теряется. Для отладки и мониторинга нужно логировать:

```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Ошибка при создании заказа");
    await _unitOfWork.RollbackTransactionAsync();
    return (OrderResult.BadRequest, null);
}
```

---

### 7. OrderService — возможный NullReference в CartService

```csharp
// CartService.GetCartAsync, строка 19
Price = item.Product.Price,
```

`GetCartAsync` подгружает `Product` через `Include`, но при удалении продукта `item.Product` может быть null. Лучше добавить проверку или использовать null-conditional.

---

### 8. Нет валидации DTO

Для `RegisterDto`, `CreateProductDto`, `LoginDto` и др. нет атрибутов валидации (`[Required]`, `[Range]`, `[EmailAddress]`, `[MinLength]` и т.п.). В контроллерах не проверяется `ModelState.IsValid`. В API можно передать пустые или некорректные данные.

---

### 9. Логирование через Console

В `Program.cs` и JWT Events используется `Console.WriteLine`. Для продакшена нужен `ILogger` и настройка провайдеров логирования.

---

### 10. Хардкод статусов заказа

Статус хранится как строка `"Pending"`. Рекомендуется ввести enum для статусов заказа и использовать его в модели и логике.

---

## Замечания по стилю и именованию

| Текущее            | Рекомендация        | Где встречается                 |
|--------------------|---------------------|----------------------------------|
| `ICartServices`    | `ICartService`      | Интерфейс сервиса корзины       |
| `UpdateQuintityAsync` | `UpdateQuantityAsync` | CartController, CartService   |
| `Reviews`          | `Review`            | Модель — обычно в единственном числе |
| `CreateAt`         | `CreatedAt`         | Модели                          |
| `wishListItems`    | `WishListItems`     | AppDbContext                    |

---

## Сильные стороны

1. Чёткое разделение слоёв: Controllers → Services → Repositories.
2. Unit of Work с поддержкой транзакций.
3. JWT-аутентификация, ASP.NET Identity, `[Authorize]`.
4. Swagger с Bearer-авторизацией.
5. Асинхронные вызовы в репозиториях и сервисах.
6. Результатные enum’ы (`CartResult`, `OrderResult`) для обработки ошибок.

---

## Рекомендации

### Приоритет 1 (срочно)

1. Добавить `await` в `OrderController.GetOrderAsync`.
2. Исправить проверку корзины в `OrderService` (до цикла).
3. Добавить `GetCartItemByProductAsync` и исправить логику добавления в корзину.
4. Убрать присвоение `product.Id` в `ProductService.PatchProduct`.

### Приоритет 2

5. Внедрить логирование (ILogger) вместо `Console.WriteLine`.
6. Добавить валидацию DTO и проверку `ModelState.IsValid`.
7. Исправить сборку `OrderDto` в `OrderService` (использовать OrderItems, а не CartItems).

### Приоритет 3

8. Ввести enum для статусов заказа.
9. Унифицировать именование (Quantity, ICartService и т.д.).
10. Добавить централизованную обработку ошибок (Exception Handler middleware).
