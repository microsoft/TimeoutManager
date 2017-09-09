# Timeout Manager
This project aims to support services with managing the timeout counting of requests.

## The Interface
There are three public members to the TimeoutManager<T> class & interface
* bool CountTimeout(T timeoutItem)
Starts the timeout count for a given object
Returns 'true' if a timeout count has started for the timeoutItem. Returns 'false' if the timeoutItem's timeout is already being counted.

* bool TryCancelTimeout(T cancelledItem)
Cancels the timeout count for the object if it has not already been timed out
Returns 'true' if the cancelledItem's timeout count was successfully cancelled, false if the cancelledItem's timeout was not being counted

* event ItemTimedOutEventHandler<T> ItemTimedOut
An event which is invoked every time an object times out
The delegate used by this event is using an EventArgs type which also has a TimedOutItem property of type T

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
