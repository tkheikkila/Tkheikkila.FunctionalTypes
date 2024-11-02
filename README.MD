Tkheikkila.FunctionalTypes
====

Useful types missing from C#

Inspired by Rust's
[`std::unit`](https://doc.rust-lang.org/std/primitive.unit.html),
[`std::option::Option`](https://doc.rust-lang.org/std/option/enum.Option.html), and 
[`std::result::Result`](https://doc.rust-lang.org/std/result/enum.Result.html).

## `Unit`

**AKA:** `void`

Contains no information other that it's type.

## `Maybe<T>`

**AKA:** `Option` or `Optional`

A value that may or may not contain a value of type `T`.

This type has two states:

* `Some` that contains a single value of type `T`  
  Created by calling `Maybe<T>.Some(value)`.
* `None` that contains no value  
  Created by calling `Maybe<T>.None()` or by returning `default`.

## `Result<TValue, TError>`

**AKA:** `Either`

A value that can be in one of two states, but never both.

These states are:

* `Ok` that indicates success with a value of type `TValue`  
  Created by calling `Result<TValue, TError>.Ok(value)`.
* `Error` that indicates failure with a value of type `TError`   
  Created by calling `Result<TValue, TError>.Error(error)`

## `MaybeResult<TValue, TError>`

A combination of `Maybe<T>` and `Result<TValue, TError>`.

Equivalent to `Maybe<Result<TValue, TError>>`

Resembles `Result<Maybe<TValue>, TError>` and `Result<TValue, Maybe<TError>>`

This type has three states:

* `Ok` that indicates success with a value of type `TValue`
  Created by calling `MaybeResult<TValue, TError>.Ok(value)`.
* `Error` that indicates failure with a value of type `TError`
  Created by calling `MaybeResult<TValue, TError>.Error(error)`.
* `Neither` that indicates that neither a value of type `TValue` nor a value of type `TError` is present
  Created by calling `MaybeResult<TValue, TError>.Neither()`.

## `GreedyValidationResult<TValue, TError>`

A value that contains a value of type `TValue` and a collection of errors of type `TError`.

Resembles `Result<TValue, List<TError>>` with a value of type `TValue` that is always present.

Resembles `LazyValidationResult<TValue, TError>` with a collection of errors of type `TError`.

This type has two states:

* `Valid` that contains no errors
* `Invalid` that contains one or more errors of type `TError`

Created by calling `Validation.Greedy<TValue, TError>(value)`.

Errors can be added by calling `AddError(error)`, one of the `AddErrors` overloads, or one of the `Validate` overloads that returns a value of `TError`.

## `LazyValidationResult<TValue, TError>`

A value that contains a value of type `TValue`. If the value is invalid, it also contains a single error of type `TError`.

Resembles `GreedyValidationResult<TValue, TError>` with a maximum of one error.

Resembles `Result<TValue, TError>` with a value of type `TValue` that is always present.

This type has two states:

* `Valid` that contains no errors
* `Invalid` that contains single error of type `TError`

Created by calling `Validation.Lazy<TValue, TError>(value)`.

Error can be set by calling one of the `Validate` overloads that returns a value of `TError`.

