Tkheikkila.FunctionalTypes
====

Useful types missing from C#

Inspired by Rust's
[`std::result::Result`](https://doc.rust-lang.org/std/result/enum.Result.html),
[`std::option::Option`](https://doc.rust-lang.org/std/option/enum.Option.html), and
[`std::unit`](https://doc.rust-lang.org/std/primitive.unit.html) respectively

## `Result<TValue, TError>`

**AKA:** `Either`

A value that contains one of two values, but never both or neither.

This type has two states:

* `Success` that contains a value of type `TValue`
* `Failure` that contains a value of type `TError`

## `Maybe<T>`

**AKA:** `Option`

A type that contains 0..1 values

This type has two states:

* `Some` that contains a single value of type `T`
* `None` that contains no value

## `Unit`

**AKA:** `void` with a value

Contains no information other that it's type.

