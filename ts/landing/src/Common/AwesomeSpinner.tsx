import * as React from "react";

export default class AwesomeSpinner extends React.Component<any, any> {
    private timer: NodeJS.Timer | undefined = undefined;

    constructor(props: any) {
        super(props);
        this.state =  {
            showSpinner: false,
        };
    }

    componentDidMount() {
        this.timer = setTimeout(
            () => this.setState({showSpinner: true}),
            this.props.delay ?? 750
        );
    }

    componentWillUnmount() {
        clearTimeout(this.timer!);
    }

    render() {
        return this.state.showSpinner && <div className="multi-spinner-container">
            <div className="multi-spinner">
                <div className="multi-spinner">
                    <div className="multi-spinner">
                        <div className="multi-spinner">
                            <div className="multi-spinner">
                                <div className="multi-spinner"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }
}
