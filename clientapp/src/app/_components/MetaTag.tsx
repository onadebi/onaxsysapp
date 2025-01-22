import React from "react";
import { Helmet } from "react-helmet-async";
import appsettings from "../common/config/appsettings";

interface IProps {
  title: string;
  description?: string;
  keywords?: string;
}

const MetaTag: React.FC<IProps> = ({
  title = `${appsettings.appName}`,
  description=`${appsettings.appName}`,
  keywords = "Onaxsys, AI, Course, Generator",
}) => {
  title = `${title} - ${appsettings.appName}`;
  return (
    <>
      <Helmet>
        <title>{title}</title>
        <meta charSet="UTF-8" />
        <meta name="description" content={description} />
        <meta name="Keywords" content={keywords} />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <link rel="canonical" href="https://onadebi.com/"></link>
      </Helmet>
    </>
  );
};

export default MetaTag;