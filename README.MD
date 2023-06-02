Tkheikkila.FunctionalTypes
====

Useful types missing from C#

Inspired by Rust's
[`std::result::Result`](https://doc.rust-lang.org/std/result/enum.Result.html),
[`std::option::Option`](https://doc.rust-lang.org/std/option/enum.Option.html), and
[`std::unit`](https://doc.rust-lang.org/std/primitive.unit.html) respectively

## `Result<TValue, TError>`

**AKA:** strict `Either`

A value that can be in one of two states, but never both or neither.

These states are:

* `Success` that indicates success with a value of type `TValue`
* `Failure` that indicates failure with a value of type `TError`

## `Result<TError>`

**AKA:** `Option` (`Maybe<TError>`) or strict `Either` without the other value (`Result<Unit, TError>`).

A value that can be in one of two states, but never both or neither.

These states are:

* `Success` that indicates success contains no value
* `Failure` that indicates failure with a value of type `TError`

## `Maybe<T>`

**AKA:** `Option`

A value that may or may not contain a value of type `T`.

This type has two states:

* `Some` that contains a single value of type `T`
* `None` that contains no value

## `Unit`

**AKA:** `void` with a value

Contains no information other that it's type.

